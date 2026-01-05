/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconShield = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M12.3664 4.06856C12.1309 3.97584 11.869 3.97584 11.6336 4.06856L5.6336 6.43124C5.25131 6.58178 5 6.95083 5 7.3617V11.0021C5 14.3248 6.83074 17.3772 9.76201 18.9418L11.5291 19.8851C11.8234 20.0421 12.1766 20.0421 12.4709 19.885L14.238 18.9418C17.1692 17.3772 18.9999 14.3248 18.9999 11.0021V7.3617C18.9999 6.95083 18.7486 6.58178 18.3663 6.43124L12.3664 4.06856ZM10.9008 2.20765C11.6072 1.92948 12.3928 1.92948 13.0992 2.20765L19.0991 4.57032C20.246 5.02194 20.9999 6.1291 20.9999 7.3617V11.0021C20.9999 15.0632 18.7624 18.7939 15.1797 20.7062L13.4127 21.6494C12.5298 22.1207 11.4702 22.1207 10.5873 21.6494L8.82023 20.7062C5.23757 18.7939 3 15.0632 3 11.0021V7.3617C3 6.1291 3.75393 5.02194 4.90081 4.57032L10.9008 2.20765Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}
export default IconShield
