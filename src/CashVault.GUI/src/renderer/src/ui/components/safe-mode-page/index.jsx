/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import MasterAuthModal from '@ui/components/master-auth-modal'
import Button from '@ui/components/button'
import { useScreenHeader } from '@src/app/domain/global/stores'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
`

const SafeModePage = () => {
  const navigate = useNavigate()
  const [masterOpen, setMasterOpen] = useState(false)
  useScreenHeader('')
  const { t } = useTranslation()

  return (
    <div>
      <Container>
        <Button onClick={() => setMasterOpen(true)}>{t('Enter safe mode')}</Button>

        <MasterAuthModal
          open={masterOpen}
          onClose={() => setMasterOpen(false)}
          onAuthenticated={() => {
            setMasterOpen(false)
            navigate('safe-mode')
          }}
        />
      </Container>
    </div>
  )
}

export default SafeModePage
