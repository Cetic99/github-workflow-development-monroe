/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconPhoneVoice from '@icons/IconPhoneVoice'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  border: 2px solid #ccd3c7;
  border-radius: 16px;
  padding: 1rem 1.5rem 1rem 1rem;
  background-color: transparent;
  width: fit-content;
  display: flex;
  gap: 0.75rem;
  align-items: center;

  & .content {
    display: flex;
    flex-direction: column;

    & .main {
      font-weight: 600;
      font-style: SemiBold;
      font-size: 0.75rem;
      line-height: 1rem;
      text-transform: uppercase;
    }

    & .secondary {
      font-weight: 700;
      font-style: Bold;
      font-size: 1.5rem;
      line-height: 2rem;
    }
  }
`

const ContactSupport = ({ number = '0800 300 600' }) => {
  const { t } = useTranslation()
  return (
    <Container>
      <IconPhoneVoice size="l" color="var(--primary-medium)" />

      <div className="content">
        <span className="main">{t('CONTACT SUPPORT')}</span>
        <span className="secondary">{number}</span>
      </div>
    </Container>
  )
}

export default ContactSupport
